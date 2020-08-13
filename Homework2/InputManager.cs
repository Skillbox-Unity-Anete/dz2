using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Homework_Theme_01
{
    public class InputManager<T>
        where T : class, new()
    {
        T instance;

        public InputManager()
        {
            instance = new T();
        }

        public void Run()
        {
            var entityName = ExtractDisplayName(instance.GetType());

            Console.WriteLine("Добро пожаловать в программу \"Записная книжка\"!");
            Console.WriteLine($"Пожалуйста, заполните информацию о {entityName}:");
            RequestFilling();
            Console.WriteLine($"Теперь вы можете выполнять операции над {entityName}:");
            RunMethod();
            Console.WriteLine($"Хотите просмотреть доступную информацию о {entityName}? Если да то нажмите любую клавишу, если нет то нажмите esc для выхода");
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                PrintObject();
                Console.WriteLine("Нажмите esc для выхода или любую другую клавишу для продолжения");
            }
        }

        public void PrintObject()
        {
            Console.WriteLine("Для того что бы определить каким образом выводить информацию выберите один из 4 способов:");
            new string[] { "Вывод", "Формат", "Интерполяция", "Центр" }.ForEach((t, i) =>
            {
                Console.WriteLine($"{i}. {t}");
            });


            var option = Console.ReadLine();
            OutputType outType = OutputType.Usuall;
            if (int.TryParse(option, out int num)) //здесь мне было лень выделять обработку в отдельную функцию, и так абстракции уже куча
            {
                outType = (OutputType)num;
            }
            else
            {
                Console.WriteLine("Невозможно определить способ вывода!");
            }

            var height = (Console.WindowHeight / 2) - InstanceProps().Length / 2;

            if (outType == OutputType.Center)
            {
                Console.Clear();
            }

            foreach (var prop in InstanceProps())
            {
                var name = ExtractDisplayName(prop);
                var value = prop.GetValue(instance);

                switch (outType)
                {
                    case OutputType.Formatting:
                        Console.WriteLine("{0} : {1}",
                            name, value);
                        break;
                    case OutputType.Interpolation:
                        Console.WriteLine($"{name} : {value}");
                        break;
                    case OutputType.Center:
                        var text = $"{name} : {value}";
                        var width = (Console.WindowWidth / 2) - (text.Length / 2);
                        Console.SetCursorPosition(width, height);
                        Console.Write(text);
                        break;
                    default: //OutputType.Usuall
                        Console.WriteLine(name + " : " + value);
                        break;
                }

                height++;
            }

            if (outType == OutputType.Center)
            {
                Console.WriteLine();
            }
        }

        public void RequestFilling()
        {
            Console.WriteLine($"Пожалуйста заполните данные о {ExtractDisplayName(instance.GetType())}:");

            foreach (var prop in InstanceProps())
            {
                Console.WriteLine($"Присвойте значние полю {ExtractDisplayName(prop)} типа {prop.PropertyType} и нажмите Enter:");
                var value = Console.ReadLine();

                var isntanceParam = Expression.Parameter(instance.GetType());

                Expression convert = default;
                try
                {
                    convert = Expression.Convert(Expression.Constant(value), prop.PropertyType);
                }
                catch (InvalidOperationException)
                {
                    var parseMethod = prop.PropertyType.GetMethod("Parse", new[] { typeof(string) });
                    convert = Expression.Call(parseMethod, Expression.Constant(value));
                }

                var assignment = Expression.Assign(Expression.Property(isntanceParam, prop.Name), convert);
                var lambda = Expression.Lambda(assignment, isntanceParam);

                lambda.Compile().DynamicInvoke(instance);
            }
        }

        private PropertyInfo[] InstanceProps() => instance.GetType().GetProperties();

        public void RunMethod()
        {
            Console.WriteLine("Выберите номер одной из доступных операций:");

            var methods = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).ToArray();

            for (int i = 0; i < methods.Length; i++)
            {
                Console.WriteLine($"{i}. {ExtractDisplayName(methods[i])}");
            }

            var count = Console.ReadLine();
            if (int.TryParse(count, out int num))
            {
                var p = Expression.Parameter(instance.GetType());
                var call = Expression.Call(p, methods[num]);
                var lambda = Expression.Lambda(call, p);

                var result = lambda.Compile().DynamicInvoke(instance);
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Невозможно определить номер операции!");
            }
        }

        private string ExtractDisplayName(object o)
        {
            string value = default;
            string safeValue = default;

            Attribute attribute = default;

            switch (o)
            {
                case MemberInfo memberInfo:
                    {
                        attribute = Attribute.GetCustomAttributes(memberInfo, typeof(DisplayNameAttribute)).FirstOrDefault();
                        safeValue = memberInfo.Name;
                    }
                    break;
                default:
                    break;
            }

            if (attribute is DisplayNameAttribute displayAttr)
            {
                value = displayAttr.DisplayName;
            }

            return value ?? safeValue;
        }
    }

    public enum OutputType
    {
        Usuall = 0,
        Formatting = 1,
        Interpolation = 2,
        Center = 3
    }
}