
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
                return false; // �������� null�̸� �� ����
            
            if (EqualityComparer<T>.Default.Equals(blackBoardKey.Key, default(T)))
                return false; // �� ������ �⺻��(0, false ��)�̸� �� ����

            return true; // ���� �ִ� ���
        }

        static public bool IsNotSet(K blackBoardKey)
        {
            if (blackBoardKey == null) return true;

            if (blackBoardKey.Key is null)
                return true; // �������� null�̸� �� ����

            if (EqualityComparer<T>.Default.Equals(blackBoardKey.Key, default(T)))
                return true; // �� ������ �⺻��(0, false ��)�̸� �� ����

            return false; // ���� �ִ� ���
        }
    }
}


