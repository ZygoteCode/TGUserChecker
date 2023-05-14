public class Utils
{
    private static char[] _allowedUsernameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    public static bool IsTelegramUsernameValid(string username)
    {
        if (username.Length < 5 || username.Length > 32)
        {
            return false;
        }

        foreach (char c in username)
        {
            bool exists = false;

            foreach (char s in _allowedUsernameCharacters)
            {
                if (c.Equals(s))
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                return false;
            }
        }

        return true;
    }
}