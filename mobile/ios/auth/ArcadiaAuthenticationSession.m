/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

#import <AuthenticationServices/AuthenticationServices.h>
#import "ArcadiaAuthenticationSession.h"

NSInteger const CANCELLATION_ERROR_CODE = 1;
NSString* const CANCELLATION_SUBCODE = @"error_subcode=cancel";

//============================================================================
@implementation ArcadiaAuthenticationSession

//----------------------------------------------------------------------------
- (dispatch_queue_t) methodQueue
{
    return dispatch_get_main_queue();
}

RCT_EXPORT_MODULE()

//----------------------------------------------------------------------------
RCT_REMAP_METHOD(getSafariData,
                 address:(NSString *)address
                 callbackURL:(NSString *)callbackURL
                 resolver:(RCTPromiseResolveBlock)resolve
                 rejecter:(RCTPromiseRejectBlock)reject)
{
    if (@available(iOS 12.0, *))
    {
        NSURL* siteURL = [NSURL URLWithString:address];
        self.authSession = [[ASWebAuthenticationSession alloc] initWithURL:siteURL
            callbackURLScheme:callbackURL
            completionHandler:^(NSURL* cbURL, NSError* error)
            {
                if (error != nil)
                {
                    NSLog(@"%@", [error localizedDescription]);

                    NSInteger errorCode = [self arcadiaErrorCodeByASWebAuthenticationSessionErrorCode:error.code];
                    reject([@(errorCode) stringValue], [error localizedDescription], error);
                    return;
                }

                if ([self isCancellationUrl:cbURL])
                {
                    reject([@(CANCELLATION_ERROR_CODE) stringValue], @"Back button pressed", nil);
                    return;
                }

                resolve(cbURL.absoluteString);
            }];

        BOOL success = [self.authSession start];

        if (success == NO)
        {
            NSError *error = [NSError errorWithDomain:@"Open safari" code:404 userInfo:nil];
            reject(@"Open safari", @"could not start opening safari", error);
        }
    }
    else
    {
        NSError *error = [NSError errorWithDomain:@"OS requirement" code:404 userInfo:nil];
        reject(@"OS requirement", @"iOS 12+ required", error);
        return;
    }
}

//----------------------------------------------------------------------------
- (NSInteger)arcadiaErrorCodeByASWebAuthenticationSessionErrorCode:(NSInteger)sessionErrorCode {
    if (sessionErrorCode == ASWebAuthenticationSessionErrorCodeCanceledLogin)
        return CANCELLATION_ERROR_CODE;

    return sessionErrorCode;
}

//----------------------------------------------------------------------------
- (BOOL) isCancellationUrl:(NSURL*)url
{
    return [[url absoluteString] containsString:CANCELLATION_SUBCODE];
}

@end
