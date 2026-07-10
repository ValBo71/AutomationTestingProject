package helpers;

import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

public class RuntimeVariables {
    private static final Map<String, Object> variables = new ConcurrentHashMap<>();

    public static void set(String key, Object value) {
        if (value != null) {
            variables.put(key, value);
        }
    }

    public static Object get(String key) {
        return variables.get(key);
    }

    public static String getString(String key) {
        Object val = variables.get(key);
        return val == null ? null : val.toString();
    }

    public static Long getLong(String key) {
        Object val = variables.get(key);
        if (val instanceof Number) {
            return ((Number) val).longValue();
        } else if (val instanceof String) {
            return Long.parseLong((String) val);
        }
        return null;
    }

    public static void remove(String key) {
        variables.remove(key);
    }

    public static void clear() {
        variables.clear();
    }
}
