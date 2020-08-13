using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_Theme_01
{
    [DisplayName("Пользователь")]
    public class User
    {
        [DisplayName("Имя")]
        public string Name { get; set; }

        [DisplayName("Возраст")]
        public int Age { get; set; }

        [DisplayName("Рост")]
        public int Height { get; set; }

        [DisplayName("Баллы по Истории")]
        public int History { get; set; }

        [DisplayName("Баллы по Математике")]
        public int Math { get; set; }

        [DisplayName("Баллы по Русскому языку")]
        public int RusLang { get; set; }

        [DisplayName("Средний балл")]
        public void AverageAmount() => Console.WriteLine("Среднее количество баллов: "+(History + Math + RusLang) / 3);
    }
}