
#include <unity.h>
#include <Arduino.h>
#include <CartReader.h>


void setUp(void)
{
    // for board bodge
    pinMode(A6, INPUT);
    pinMode(A7, INPUT);

    pinMode(ADDRESS_REGISTER_CLEAR, OUTPUT);
    pinMode(ADDRESS_REGISTER_CLOCK, OUTPUT);
    pinMode(ADDRESS_REGISTER_LATCH, OUTPUT);
    pinMode(ADDRESS_REGISTER_ENABLE, OUTPUT);
    pinMode(ADDRESS_REGISTER_DATA, OUTPUT);

    pinMode(CART_DATA_0, INPUT);
    pinMode(CART_DATA_1, INPUT);
    pinMode(CART_DATA_2, INPUT);
    pinMode(CART_DATA_3, INPUT);
    pinMode(CART_DATA_4, INPUT);
    pinMode(CART_DATA_5, INPUT);
    pinMode(CART_DATA_6, INPUT);
    pinMode(CART_DATA_7, INPUT);

    pinMode(CART_CLOCK, OUTPUT);
    pinMode(CART_WRITE_ENABLE, OUTPUT);
    pinMode(CART_READ_ENABLE, OUTPUT);
    pinMode(CART_CHIP_SELECT, OUTPUT);
    pinMode(CART_RESET, OUTPUT);
    digitalWrite(CART_RESET, HIGH);

    digitalWrite(ADDRESS_REGISTER_CLEAR, HIGH);
    digitalWrite(ADDRESS_REGISTER_CLOCK, LOW);
    digitalWrite(ADDRESS_REGISTER_LATCH, LOW);
    digitalWrite(ADDRESS_REGISTER_ENABLE, LOW);
    digitalWrite(ADDRESS_REGISTER_DATA, LOW);
}

void tearDown(void) {
    // clean stuff up here
}

void test_request(void)
{
    CartReader cart;
    
    TEST_ASSERT(cart.SanityCheck());

}

int main(int argc, char **argv)
{
    UNITY_BEGIN();

    RUN_TEST(test_request);

    return UNITY_END();
}