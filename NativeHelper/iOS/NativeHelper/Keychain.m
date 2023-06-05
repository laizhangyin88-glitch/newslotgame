#import "Keychain.h"
#import "KeychainItemWrapper.h"

@implementation Keychain

+ (NSString *) udidUsingCFUUID
{
    // initialize keychaing item for saving UUID.
    KeychainItemWrapper *wrapper = [[KeychainItemWrapper alloc] initWithIdentifier:@"UUID" accessGroup:@"EQSL82W24G.slots"];

    NSString *uuid = [wrapper objectForKey:(__bridge id)kSecAttrAccount];
    if ( uuid == nil || uuid.length == 0)
    {
        // for backward compatibility, try to read from private keychain
        KeychainItemWrapper *privateWrapper = [[KeychainItemWrapper alloc] initWithIdentifier:@"UUID" accessGroup:nil];
        uuid = [privateWrapper objectForKey:(__bridge id)kSecAttrAccount];
        
        if (uuid == nil || uuid.length == 0) {
            // if there is not UUID in keychain, make UUID.
            CFUUIDRef uuidRef = CFUUIDCreate(NULL);
            CFStringRef uuidStringRef = CFUUIDCreateString(NULL, uuidRef);
            CFRelease(uuidRef);
            uuid = [NSString stringWithString:(__bridge NSString *) uuidStringRef];
            CFRelease(uuidStringRef);
        }

        // save UUID in keychain
        [wrapper setObject:uuid forKey:(__bridge id)kSecAttrAccount];
    }

    return uuid;
}

@end
