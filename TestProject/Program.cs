
using System.Text;
using System.Linq;
using System.Collections;

class Program
{

    public static bool HasValue<T>(T value)
    {
        if (value is null)
            return false; // 참조형이 null이면 값 없음

        if (EqualityComparer<T>.Default.Equals(value, default(T)))
            return false; // 값 형식이 기본값(0, false 등)이면 값 없음

        return true; // 값이 있는 경우
    }

    public class Bee()
    {

    }

    public static void ConfirmValue(ref Object arg)
    {
        arg = 15;
    }

    static void Main(string[] args)
    {
        string monsterName = "Mage";


        double result = Math.Pow(0 * 0 + 0 * 0, 0.5f);

    }
}

