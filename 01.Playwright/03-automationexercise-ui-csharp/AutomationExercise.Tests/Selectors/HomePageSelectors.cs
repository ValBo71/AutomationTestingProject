namespace AutomationExercise.Tests.Selectors
{
    public static class HomePageSelectors
    {
        public const string SubscriptionEmailInput = "#susbscribe_email";
        public const string SubscriptionButton = "#subscribe";
        public const string SubscriptionSuccessMessage = "#success-subscribe"; // text matches success
        public const string SliderCarousel = "#slider-carousel";
        public const string RecommendedItemsHeader = "div.recommended_items h2";
        public const string FirstRecommendedItemAddToCart = "div.recommended_items .carousel-inner .active .col-sm-4:first-child a.add-to-cart";
    }
}
