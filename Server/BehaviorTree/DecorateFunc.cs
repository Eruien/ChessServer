
namespace ServerContent
{
    public class DecorateFunc<T> where T : IComparable
    {
        static public bool IsEqualTo(T keyValue, T blackBoardKey)
        {
            if (blackBoardKey.CompareTo(keyValue) == 0)
            {
                return true;
            }

            return false;
        }

        static public bool IsNotEqualTo(T keyValue, T blackBoardKey)
        {
            if (blackBoardKey.CompareTo(keyValue) != 0)
            {
                return true;
            }

            return false;
        }

        static public bool IsLessThan(T keyValue, T blackBoardKey)
        {
            if (blackBoardKey.CompareTo(keyValue) < 0)
            {
                return true;
            }

            return false;
        }

        static public bool IsLessThanOrEqualTo(T keyValue, T blackBoardKey)
        {
            if (blackBoardKey.CompareTo(keyValue) <= 0)
            {
                return true;
            }

            return false;
        }

        static public bool IsGreaterThan(T keyValue, T blackBoardKey)
        {
            if (blackBoardKey.CompareTo(keyValue) > 0)
            {
                return true;
            }

            return false;
        }

        static public bool IsGreaterThanOrEqualTo(T keyValue, T blackBoardKey)
        {
            if (blackBoardKey.CompareTo(keyValue) >= 0)
            {
                return true;
            }

            return false;
        }
    }

    public class SetDecorateFunc<T, K> where K : IBlackBoardKey<T>
    {
        static public bool IsSet(K blackBoardKey)
        {
            if (blackBoardKey == null) return false;

            if (blackBoardKey.Key is null)
                return false; // 참조형이 null이면 값 없음
            
            if (EqualityComparer<T>.Default.Equals(blackBoardKey.Key, default(T)))
                return false; // 값 형식이 기본값(0, false 등)이면 값 없음

            return true; // 값이 있는 경우
        }

        static public bool IsNotSet(K blackBoardKey)
        {
            if (blackBoardKey == null) return true;

            if (blackBoardKey.Key is null)
                return true; // 참조형이 null이면 값 없음

            if (EqualityComparer<T>.Default.Equals(blackBoardKey.Key, default(T)))
                return true; // 값 형식이 기본값(0, false 등)이면 값 없음

            return false; // 값이 있는 경우
        }
    }
}


