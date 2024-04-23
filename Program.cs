//file Program.cs

using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpSys3
{
    class Program
    {
        public static void Main()
        {
            List<int> need = new List<int>() { 8, 8, 9, 2, 1, 0, 8, 9, 2, 4, 6, 8, 2, 1, 8, 9 }; //список обращений
            List<int> data = new List<int>() { 8, 2, 9, 6 }; //список используемыъ страниц
            int count = 2; //количество циклов
            List<Tuple<int, string>> tuples = new List<Tuple<int, string>>();
            for (int i = 0; i < count; i++) //два цикла выталкивания
            {
                tuples.Add(fifo(data, need)); //fifo
                tuples.Add(lru(data, need)); //lru
                data.Add(-1); //после первого прохода добавление пятой пустой страницы
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            tuples.Sort((x, y) => x.Item1.CompareTo(y.Item1)); //сортировка по количеству прерываний
            string info = "Результаты:\n";
            for (int i = 0; i < tuples.Count; i++) //вывод результатов для каждого цикла
                info += $"{tuples[i].Item2} количество прерываний - {tuples[i].Item1}\n"; //вывод количества прерываний
            info += $"Самый эффективный алгоритм для исходных данных: {tuples.First().Item2.Trim(':')}. Количество прерываний: {tuples.First().Item1}"; //вывод самого эффективного
            Console.WriteLine(info); //вывод результатов
            Console.ResetColor();
            return;
        }

        public static List<int> insert(ref List<int> list, int ins)
        {
            if (list.Last() == -1) //если последняя страница пустая
            {
                list[list.Count - 1] = ins; //то добавить в конец
                return list;
            }
            for (int i = 0; i < list.Count - 1; i++) //иначе вставить в нужную позицию
                list[i] = (list[i + 1]);
            list[list.Count - 1] = ins;
            return list;
        }

        public static string make(List<int> list) // преобразование страниц в строку
        {
            string print = "";
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != -1)
                    print += $"{list[i]}"; //если не пустой, то вывод
                else
                    print += "empty"; //иначе добавить empty
                if (i != list.Count - 1)
                    print += "-"; //добавить разделитель
            }
            return print;
        }

        public static List<int> lruIns(List<int> list, int ins)
        {
            int i = 0;
            if (list.Last() != -1) //если последняя страница не пустая
            {
                do
                {
                    if (list[i] == ins) //если нашли нужную страницу
                        (list[i], list[i + 1]) = (list[i + 1], list[i]); //то меняем местами
                    i++;
                }
                while (list.Last() != ins); //пока не дошли до нужной страницы
            }
            else //если последняя страница пустая
            {
                do
                {
                    if (list[i] == ins) //если нашли нужную страницу
                        (list[i], list[i + 1]) = (list[i + 1], list[i]); //то меняем местами
                    i++;
                }
                while (list[list.Count - 2] != ins); //пока не дошли до нужной страницы
            }
            return list;
        }

        public static Tuple<int, string> fifo(List<int> first, List<int> need)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string info = $"Алгоритм FIFO для {first.Count} страниц:";
            Console.WriteLine(info);
            Console.WriteLine("");
            List<int> data = new List<int>(first);

            int stop = 0;
            string output = "";
            for (int i = 0; i < need.Count; i++) //проход по страницам
            {
                string print = $"{i + 1}. "; //выод номера цикла 
                if (!data.Contains(need[i])) //если не содержится
                {
                    stop++; //увеличение количества прерываний
                    print += "Прерывание: ";
                    print += make(data); //вывод изначального состояния страниц
                    print += "  //  "; //добавить разделитель
                    insert(ref data, need[i]); //вставить в конец
                    print += make(data); //вывод изменённого состояния страниц
                }
                else //если содержится
                    print += $"Обращение к странице {need[i]} из блоков {make(data)}"; //вывод к какой странице обращение
                print += "\n";
                output += print;
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(output);
            Console.WriteLine($"\nКоличество прерываний: {stop}\n\n"); //вывод количества прерываний
            Tuple<int, string> tuple = new Tuple<int, string>(stop, info); //количество прерываний и сформированная строка информации
            return tuple;
        }

        public static Tuple<int, string> lru(List<int> first, List<int> need)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan; //цвет
            string info = $"Алгоритм LRU для {first.Count} страниц:"; //количество страниц
            Console.WriteLine(info); //вывод количества страниц
            Console.WriteLine("");
            List<int> data = new List<int>(first); //список с используемыми страницами
            int stop = 0;
            string output = "";
            for (int i = 0; i < need.Count; i++) //цикл пока не выполнены все обращения
            {
                string print = $"{i + 1}. "; //вывод номера цикла
                if (!data.Contains(need[i])) //если страница не используется
                {
                    stop++; //увеличение счетчика прерываний
                    print += "Прерывание: ";
                    print += make(data);  //вывод изначального состояния страниц
                    print += "  -->  ";
                    insert(ref data, need[i]); //добавление страницы в список
                    print += make(data); //вывод текущего состояния страниц
                }
                else  //если страница используется
                {
                    print += $"Обращение к странице {need[i]}"; //вывод к какой странице обращение
                    if (data[data.Count - 1] != need[i]) //если нужная не последняя страница
                    {
                        if (data[data.Count - 1] == -1 && data[data.Count - 2] == need[i]) //если последняя страница пустая и предпоследняя - нужная
                            print += $" из блоков {make(data)}"; //вывод изначального состояния страниц
                        else
                            print += $".\n   Блоки обновлены: {make(data)} --> {make(lruIns(data, need[i]))}"; //вывод изменённого состояния страниц

                    }
                    else
                        print += $" из блоков {make(data)}"; //если нужная последняя страница
                }

                print += "\n";
                output += print;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(output);
            Console.WriteLine($"\nКоличество прерываний: {stop}\n\n"); //вывод количества прерываний
            Tuple<int, string> tuple = new Tuple<int, string>(stop, info); //количество прерываний и сформированная строка информации
            return tuple;
        }
    }
}