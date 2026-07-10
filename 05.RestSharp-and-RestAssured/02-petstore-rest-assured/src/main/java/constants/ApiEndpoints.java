package constants;

public final class ApiEndpoints {
    public static final String PET = "/pet";
    public static final String PET_BY_ID = "/pet/{petId}";
    
    public static final String STORE_ORDER = "/store/order";
    public static final String STORE_ORDER_BY_ID = "/store/order/{orderId}";
    public static final String STORE_INVENTORY = "/store/inventory";
    
    public static final String USER = "/user";
    public static final String USER_BY_USERNAME = "/user/{username}";
    public static final String USER_LOGIN = "/user/login";
    public static final String USER_LOGOUT = "/user/logout";

    private ApiEndpoints() {}
}
