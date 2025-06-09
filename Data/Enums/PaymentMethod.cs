namespace Data;

/// <summary>
/// Represents the method of payment used for an order
/// </summary>
public enum PaymentMethod
{
    /// <summary>Payment made with physical cash</summary>
    Cash,

    /// <summary>Payment made with a credit card</summary>
    CreditCard,

    /// <summary>Payment made with a debit card</summary>
    DebitCard,

    /// <summary>Payment made with a digital wallet app</summary>
    DigitalWallet
}