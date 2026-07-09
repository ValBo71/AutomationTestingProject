namespace AutomationExercise.Tests.Selectors
{
    public static class CartPageSelectors
    {
        // Cart Table Selectors
        public const string CartItems = "tr[id^='product-']";
        public const string CartItemRemoveButton = "a.cart_quantity_delete";
        public const string CartItemQuantity = "button.disabled";
        public const string CartItemPrice = "td.cart_price p";
        public const string CartItemTotalPrice = "td.cart_total p";
        public const string ProceedToCheckoutButton = "a.check_out";
        public const string EmptyCartContainer = "span#empty_cart";
        
        // Checkout Form Selectors
        public const string AddressDeliveryList = "#address_delivery";
        public const string AddressInvoiceList = "#address_invoice";
        public const string CheckoutCommentInput = "textarea[name='message']";
        public const string PlaceOrderButton = "a[href='/payment']";
        
        // Payment Form Selectors
        public const string PaymentNameOnCard = "input[name='name_on_card']";
        public const string PaymentCardNumber = "input[name='card_number']";
        public const string PaymentCvc = "input[name='cvc']";
        public const string PaymentExpiryMonth = "input[name='expiry_month']";
        public const string PaymentExpiryYear = "input[name='expiry_year']";
        public const string PayAndConfirmButton = "button[data-qa='pay-button']";
        public const string OrderSuccessMessage = "p:has-text('Congratulations! Your order has been confirmed!')";
        public const string DownloadInvoiceButton = "a:has-text('Download Invoice')";
        public const string OrderContinueButton = "a[data-qa='continue-button']";
    }
}
