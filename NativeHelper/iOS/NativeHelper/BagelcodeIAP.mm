//
//  BagelcodeIAP.mm
//  Unity-iPhone
//
//  Created by Sangmin Lee on 1/24/17.
//
//

#import "BagelcodeIAP.h"
 
@implementation BagelcodeIAP

// typedef NS_ENUM(NSInteger, ErrorCode) {
//     ErrorProductEmpty = 9,
//     ErrorTransactionStateDefault = 10,
//     ErrorTransactionFailed = 11,
//     ErrorNoReceipt = 12
// };

+ (id) sharedManager
{
    static BagelcodeIAP * sharedMyManager = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedMyManager = [[self alloc] init];
    });

    return sharedMyManager;
}
 
- (id) init {
    if (self == [super init]) {
        self.unityObject = [[NSMutableString alloc] initWithString:@""];
        self.unityMethodName = [[NSMutableString alloc] initWithString:@""];
        self.purchaseCurrencyCode = @"";
        self.purchaseLocalPrice = @"";
    }
     
    return self;
}
 
- (void) dealloc {
    // Should never be called, but just here for clarity really.
}
 
- (void) callDelegateSuccess:(NSData *) receipt {
    // NSLog(@"callDelegateSuccess");
    NSString *encReceipt = [receipt base64EncodedStringWithOptions:0];
    NSString *responseData = [NSString stringWithFormat:@"%@|%@|%@", encReceipt, self.purchaseCurrencyCode, self.purchaseLocalPrice];
    // NSLog(@"SKPaymentTransactionStatePurchased encReceipt : %@", encReceipt);
    // NSLog(@"SKPaymentTransactionStatePurchased responseData : %@", responseData);
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [responseData UTF8String]);
}
 
- (void) callDelegateFail:(NSString *) reason {
    // NSLog(@"callDelegateFail");
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [[NSString stringWithFormat:@"%@%@", @"Error", reason] UTF8String]);
}

- (BOOL) canMakePayments
{
    return [SKPaymentQueue canMakePayments];
}

//
// Restoring Purchased Products
//

- (void) restoreCompletedTransactions
{
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

//
// Retrieving Product Information
//

- (void) paymentRequestWithProductIdentifiers:(NSArray *) productIdentifiers
{
    if ([self canMakePayments]) {
        SKProductsRequest *productsRequest = [[SKProductsRequest alloc]
                                              initWithProductIdentifiers:[NSSet setWithArray:productIdentifiers]];

        productsRequest.delegate = self;
        [productsRequest start];
    } else {
        NSLog(@"Activate in-app purchase");
    }
}

// 
// Requesting Payment: SKProductsRequestDelegate protocol method
//

- (void) productsRequest:(SKProductsRequest *) request didReceiveResponse:(SKProductsResponse *) response
{
    for (NSString *invalidId in response.invalidProductIdentifiers)
        NSLog(@"StoreKit: invalid productIdentifier: %@", invalidId);
   
    if ([response.products count] > 0) {
        [self paymentRequest:[response.products objectAtIndex:0]];
    } else {
        NSLog(@"In-App Purchase Fail");
        // [self callDelegateFail:[NSString stringWithFormat:@"%zd", (long)ErrorProductEmpty]];
        [self callDelegateFail:[NSString stringWithFormat:@"%@", @"In-App Purchase Fail"]];
    }
}

- (void) productsRequest:(SKProductsRequest *) request didFailWithError:(NSError *) error
{
    NSString* errorStr = [NSString stringWithFormat:@"Product request failed. error description: %@", [error localizedDescription]];
    NSLog(@"%@", errorStr);
    UnitySendMessage([self.unityObject UTF8String], [self.unityMethodName UTF8String], [[NSString stringWithFormat:@"%@%@", @"Error", errorStr] UTF8String]);
}
 
- (void) paymentRequest:(SKProduct *) product
{
    SKMutablePayment *payment = [SKMutablePayment paymentWithProduct:product];
    payment.quantity = 1;

    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
    [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
    [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    [numberFormatter setLocale:product.priceLocale];
    self.purchaseLocalPrice = [numberFormatter stringFromNumber:product.price];
    self.purchaseCurrencyCode = product.priceLocale.currencyCode;

    // NSLog( @"paymentRequest formattedString.price is %@", self.purchaseLocalPrice );
    // NSLog( @"paymentRequest priceLocale.currencyCode is %@", self.purchaseCurrencyCode  );
    // NSLog( @"paymentRequest storefront.countryCode is %@", [SKPaymentQueue defaultQueue].storefront.countryCode  );
 
    [[SKPaymentQueue defaultQueue] addPayment:payment];
    
    // Now you can see payment UI. Using SKPaymentTransactionObserver, handle it properly.
}

//
// Delivering Products
//
 
// Sent when the transaction array has changed (additions or state changes).  Client should check state of transactions and finish as appropriate.
- (void) paymentQueue:(SKPaymentQueue *) queue updatedTransactions:(NSArray *) transactions {
    NSLog(@"paymentQueue");
    for (SKPaymentTransaction *transaction in transactions) {
        switch (transaction.transactionState) {
                // Call the appropriate custom method for the transaction state.
            case SKPaymentTransactionStatePurchasing:
                NSLog(@"SKPaymentTransactionStatePurchasing");
                [self showTransactionAsInProgress:transaction deferred:NO];
                break;
            case SKPaymentTransactionStateDeferred:
                NSLog(@"SKPaymentTransactionStateDeferred");
                [self showTransactionAsInProgress:transaction deferred:YES];
                break;
            case SKPaymentTransactionStateFailed:
                NSLog(@"SKPaymentTransactionStateFailed");
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStatePurchased:
                // Load the receipt from the app bundle.
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                // For non-consumable
                NSLog(@"SKPaymentTransactionStateRestored");
                [self restoreTransaction:transaction];
                break;
            default:
                // For debugging
                NSLog(@"Unexpected transaction state %@", @(transaction.transactionState));
                [self callDelegateFail:[NSString stringWithFormat:@"paymentQueue %@", [transaction.error localizedDescription]]];
                // [self callDelegateFail:[NSString stringWithFormat:@"%zd", (long)ErrorTransactionStateDefault]];
                break;
        }
    }
}
 
- (void) showTransactionAsInProgress:(SKPaymentTransaction *) transaction deferred:(BOOL) isDeferred {

}
 
- (void) failedTransaction:( SKPaymentTransaction *) transaction {
    if (transaction.error.domain == SKErrorDomain)
    {
        [self callDelegateFail:[NSString stringWithFormat:@"%zd %@", transaction.error.code, [transaction.error localizedDescription]]];
    }
    else
    {
        [self callDelegateFail:[NSString stringWithFormat:@"failedTransaction %@", [transaction.error localizedDescription]]];
        // [self callDelegateFail:[NSString stringWithFormat:@"%zd", (long)ErrorTransactionFailed]];
    };
    NSLog(@"%@", [transaction.error localizedDescription]);
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}
 
//
// Receipt Validation Programming Guide
//

- (void) completeTransaction:(SKPaymentTransaction *) transaction {
    NSURL *receiptURL = [[NSBundle mainBundle] appStoreReceiptURL];
    NSData *receipt = [NSData dataWithContentsOfURL:receiptURL];
    if (!receipt) {
        /* No local receipt -- handle the error. */
        // [self callDelegateFail:[NSString stringWithFormat:@"%zd", (long)ErrorNoReceipt]];
        [self callDelegateFail:[NSString stringWithFormat:@"%@", @"completeTransaction No Receipt"]];
    } else {
        [self callDelegateSuccess:receipt];
        [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    }
}
 
- (void) restoreTransaction:(SKPaymentTransaction *) transaction {
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}
 
// Sent when transactions are removed from the queue (via finishTransaction:).
- (void) paymentQueue:(SKPaymentQueue *) queue removedTransactions:(NSArray *) transactions {
    NSLog(@"removedTransactions");
}
 
// Sent when an error is encountered while adding transactions from the user's purchase history back to the queue.
- (void) paymentQueue:(SKPaymentQueue *) queue restoreCompletedTransactionsFailedWithError:(NSError *) error {
    NSLog(@"restoreCompletedTransactionsFailedWithError");
}
 
// Sent when all transactions from the user's purchase history have successfully been added back to the queue.
- (void) paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *) queue {
    NSLog(@"paymentQueueRestoreCompletedTransactionsFinished");

    NSMutableArray *purchasedItemIDs = [[NSMutableArray alloc] init];
    NSLog(@"received restored transactions: %i", queue.transactions.count);

    // 결재 기록이 없을때 alert 뛰우기
    if (queue.transactions.count == 0) {
        // NSString *fileMessage = NSLocalizedString(@"NOTRESTORE", @"restore");
        UIAlertView *resultView = [[UIAlertView alloc] initWithTitle:@"Failed"
                                                       message:@"There is no record of your purchase" 
                                                       delegate:self 
                                                       cancelButtonTitle:nil 
                                                       otherButtonTitles:@"OK", nil];
        [resultView show];
    } else {
        for (SKPaymentTransaction *transaction in queue.transactions)
        {
            NSString *productID = transaction.payment.productIdentifier;
            [purchasedItemIDs addObject:productID];
            NSLog (@"product id is %@" , productID);

            [self callDelegateSuccess:transaction.transactionReceipt];

            // Here put an if/then statement to write files based on previously purchased items
            // example if ([productID isequaltostring: @"youruniqueproductidentifier]){write files} else { nslog sorry}
        }
    }
}
 
// Sent when the download state has changed.
- (void) paymentQueue:(SKPaymentQueue *) queue updatedDownloads:(NSArray *) downloads {
    NSLog(@"updatedDownloads");
}
 
@end

//
// Extern Part, C Level
//
//------------------------------------------------------------------------------------

extern "C"
{
    void _init()
    {
        [BagelcodeIAP sharedManager];
    }
    
    void _initializeBagelcodeIAP(const char* unithObjectName)
    {
        NSLog(@"InitializeBagelcodeIAP");
        [[[BagelcodeIAP sharedManager] unityObject] setString:[NSString stringWithUTF8String:unithObjectName]];
    }
    
    void _paymentRequestWithProductIdentifiers(const char* productIdentifiers, const char* unityMethodName)  
    {
        // Create NSArray from const char*
        NSString *input = [[NSString alloc] initWithUTF8String: productIdentifiers];
        NSArray *result = [[NSArray alloc] initWithObjects: input, nil];

        [[[BagelcodeIAP sharedManager] unityMethodName] setString:[NSString stringWithUTF8String:unityMethodName]];
        [[BagelcodeIAP sharedManager] paymentRequestWithProductIdentifiers: result];
    }

    void _restorePurchasedProducts()
    {
        [[BagelcodeIAP sharedManager] restoreCompletedTransactions];
    }
    
}
