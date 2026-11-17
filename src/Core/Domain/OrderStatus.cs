namespace Core.Domain;

public enum OrderStatus
{
    Draft,      // Чернетка (можна додавати товари)
    Confirmed,  // Підтверджено (змінювати склад не можна, можна лише скасувати)
    Cancelled   // Скасовано (термінальний стан, жодні дії неможливі)
}