package base;

import clients.PetApiClient;
import clients.StoreApiClient;
import clients.UserApiClient;
import org.junit.jupiter.api.BeforeAll;

public class BaseApiTest {
    protected static PetApiClient petApiClient;
    protected static StoreApiClient storeApiClient;
    protected static UserApiClient userApiClient;

    @BeforeAll
    public static void setUpSuite() {
        petApiClient = new PetApiClient();
        storeApiClient = new StoreApiClient();
        userApiClient = new UserApiClient();
    }
}
