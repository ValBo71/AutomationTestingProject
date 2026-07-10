# Explanation of the Task

The goal of this test is to verify the correct execution of a set of manually conducted tests used for regression testing.
Using API capabilities, we verify whether the entered values are correctly reflected in the response received from the API.

Only a portion of the tests is uploaded here to maintain confidentiality.

## How the Test Works

1. Cases are created in iMX using XML Schemas.

2. Using an Excel script, I extract all the necessary test data from the XML schemas.
   The script generates a `*.csv` file.
3. I use the Postman Collection Runner features to load all the data from the `*.csv` file into the collection variables.
4. Once the data is loaded, the remaining test suites can be executed either collectively or individually.