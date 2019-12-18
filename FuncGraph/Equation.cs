using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncGraph
{
    class Equation
    {
        private string Infix; //rownanie w konwencjonalnym zapisie algebraicznym - infixowym
        private Queue<string> Postfix; //rownanie po przekladzie do odwrotnej notacji polskiej - zapis postfixowy

        public Equation(string equation)
        {
            this.Infix = equation;
            Converse(Tokenize(this.Infix));
        }

        private Queue<string> Tokenize(string text) //tokenizacja - podzial poszczegolnych elementow stringa wejsciowego, na liczby, operatory, funkcje matematyczne
        {
            string temp = "";
            double number;
            Queue<string> queue = new Queue<string>();

            foreach(char c in text) //czytanie wejscia znak po znaku
            {
                if (c == ' ') //ignorowanie bialych znakow
                    continue;

                switch (c)
                {
                    case '+': //odczytywanie operatorow
                    case '-':
                    case '*':
                    case '/':
                    case '^':
                    case '(':
                    case ')':
                        if(double.TryParse(temp, out number))
                        {
                            queue.Enqueue(temp);
                            temp = "";
                        }
                        else if(temp != "")
                        {
                            queue.Enqueue(temp);
                            temp = "";
                        }
                        queue.Enqueue(c.ToString());
                        break;

                    case '0': //odczytywanie cyfr oraz separatorow
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                    case ',':
                        temp += c.ToString();
                        break;

                    default: //odczytywanie funkcji
                        if(Char.IsLetter(c))
                        {
                            if (double.TryParse(temp, out number))
                            {
                                queue.Enqueue(temp);
                                queue.Enqueue("*");
                                temp = "";
                            }

                            temp += c.ToString();
                        }
                        break;
                }
            }

            if(temp != "") //umieszczenie ostatniego odczytanego elementu do kolejki przechowujacej wyrazenie w postaci infixowej
            {
                queue.Enqueue(temp);
            }

            return queue;
        }

        private int Priory(string operat) //ustalenie priorytetow poszczegolnych operatorow - kolejnosc wykonywania dzialan
        {
            switch(operat)
            {
                case "(":
                    return 0;
                case "+":
                case "-":
                case ")":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                case "sqrt":
                    return 3;
                case "sin":
                case "cos":
                case "tg":
                case "ctg":
                    return 4;
            }

            return -1;
        }
        private void Converse(Queue<string> infix) //konwersja zapisu infix do postfix
        {
            string temp;

            Queue<string> queue = new Queue<string>(); //wg algorytmu konwersji tworzona jest kolejka oraz stos
            Stack<string> stack = new Stack<string>();

            while(infix.Any()) //algorytm konwersji
            {
                temp = infix.Dequeue();
                switch(Priory(temp))
                {
                    case -1:
                        queue.Enqueue(temp);
                        break;
                    case 0:
                        stack.Push(temp);
                        break;
                    case 1:
                        if(temp == ")")
                        {
                            while(stack.Peek() != "(")
                            {
                                queue.Enqueue(stack.Pop());
                            }
                            stack.Pop();
                        }
                        else
                        {
                            while(stack.Any() && Priory(temp) <= Priory(stack.Peek()))
                            {
                                queue.Enqueue(stack.Pop());
                            }
                            stack.Push(temp);
                        }
                        break;
                    case 2:
                    case 3:
                    case 4:
                        if(temp == "^") //potega jest wyjatkiem poniewaz jest operatorem prawostronnie łącznym tzn. 3^2^4 = 3^(2^4)
                        {
                            while(stack.Any() && Priory(temp) < Priory(stack.Peek()))
                            {
                                queue.Enqueue(stack.Pop());
                            }
                        }
                        else
                        {
                            while (stack.Any() && Priory(temp) <= Priory(stack.Peek()))
                            {
                                queue.Enqueue(stack.Pop());
                            }
                        }
                        stack.Push(temp);
                        break;
                }
            }

            while(stack.Any())
            {
                queue.Enqueue(stack.Pop());
            }

            this.Postfix = queue;
        }

        public double Compute(double x) //wyliczenie wartosci funkcji dla konkretnej wartosci x
        {
            Queue<string> queue = new Queue<string>(this.Postfix);
            Stack<double> stack = new Stack<double>();

            double number;

            while(queue.Any())
            {
                switch (Priory(queue.Peek()))
                {
                    case -1:
                        if (Char.IsLetter(queue.Peek()[0]))
                        {
                            queue.Dequeue();
                            stack.Push(x);
                        }
                        else
                        {
                            stack.Push(double.Parse(queue.Dequeue()));
                        }
                        break;
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        if (queue.Peek() == "sqrt")
                        {
                            stack.Push(Math.Sqrt(stack.Pop()));
                        }
                        else
                        {
                            number = stack.Pop();
                            switch (queue.Peek())
                            {
                                case "+":
                                    number += stack.Pop();
                                    break;
                                case "-":
                                    number = stack.Pop() - number;
                                    break;
                                case "*":
                                    number *= stack.Pop();
                                    break;
                                case "/":
                                    number = stack.Pop() / number;
                                    break;
                                case "^":
                                    number = Math.Pow(stack.Pop(), number);
                                    break;
                            }
                            stack.Push(number);
                        }
                        queue.Dequeue();
                        break;
                    case 4:
                        number = stack.Pop();
                        switch (queue.Peek())
                        {
                            case "sin":
                                stack.Push(Math.Sin(number));
                                break;
                            case "cos":
                                stack.Push(Math.Cos(number));
                                break;
                            case "tg":
                                stack.Push(Math.Tan(number));
                                break;
                            case "ctg":
                                number = 1 / Math.Tan(number);
                                stack.Push(number);
                                break;
                        }
                        queue.Dequeue();
                        break;
                }
            }
            
            return stack.Pop();
        }
    }
}
