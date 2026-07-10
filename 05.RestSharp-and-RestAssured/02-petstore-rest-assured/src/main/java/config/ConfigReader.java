package config;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.io.InputStream;

public class ConfigReader {
    private static JsonNode appSettings;
    private static JsonNode collectionVariables;

    static {
        ObjectMapper mapper = new ObjectMapper();
        try (InputStream appStream = ConfigReader.class.getClassLoader().getResourceAsStream("appsettings.json")) {
            if (appStream != null) {
                appSettings = mapper.readTree(appStream);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        try (InputStream collStream = ConfigReader.class.getClassLoader().getResourceAsStream("collectionVariables.json")) {
            if (collStream != null) {
                collectionVariables = mapper.readTree(collStream);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static String getBaseUrl() {
        return appSettings != null && appSettings.has("baseUrl") ? appSettings.get("baseUrl").asText() : "https://petstore.swagger.io";
    }

    public static String getCollectionVariable(String key) {
        if (collectionVariables != null && collectionVariables.has("collectionVariables")) {
            JsonNode vars = collectionVariables.get("collectionVariables");
            if (vars.has(key)) {
                return vars.get(key).asText();
            }
        }
        return null;
    }
}
