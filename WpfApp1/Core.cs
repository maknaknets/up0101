using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class Core
    {
        // Контекст базы данных (название CinemaDBEntities может отличаться, проверь в Model.Context.cs)
        public static CinemaDBPR14Entities DB = new CinemaDBPR14Entities();

        // Хранение текущего авторизованного пользователя
        public static Users CurrentUser;
    }
}
