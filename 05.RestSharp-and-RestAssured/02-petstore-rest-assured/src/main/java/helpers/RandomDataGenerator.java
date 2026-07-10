package helpers;

import java.util.Random;

public class RandomDataGenerator {
    private static final Random random = new Random();

    public static Long generateId() {
        return 100000L + random.nextInt(900000);
    }

    public static String generateUsername() {
        return "user_" + System.currentTimeMillis();
    }

    public static String generateEmail() {
        return "email_" + System.currentTimeMillis() + "@example.com";
    }

    public static String generatePhone() {
        return "1" + (100000000L + random.nextInt(900000000));
    }
}
