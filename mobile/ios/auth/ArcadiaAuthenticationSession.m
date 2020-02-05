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
                 address: (NSString*)address
                 callbackURL: (NSString*)callbackURL
                 resolver: (RCTPromiseResolveBlock)resolve
                 rejecter: (RCTPromiseRejectBlock)reject)
{
    NSURL* siteURL = [NSURL URLWithString:address];

    if (@available(iOS 12.0, *))
    {
        self.asWebAuthSession = [[ASWebAuthenticationSession alloc] initWithURL:siteURL
            callbackURLScheme:callbackURL
            completionHandler:^(NSURL* callbackURL, NSError* error)
            {
                [self processResponseWithUrl:callbackURL error:error
                    resolver:resolve rejecter:reject];
            }];
    }
    else if (@available(iOS 11.0, *))
    {
        self.sfAuthSession = [[SFAuthenticationSession alloc] initWithURL:siteURL
            callbackURLScheme:callbackURL
            completionHandler:^(NSURL* callbackURL, NSError* error)
            {
                [self processResponseWithUrl:callbackURL error:error
                    resolver:resolve rejecter:reject];
            }];
    }
    else
    {
        NSError* error = [NSError errorWithDomain:@"OS requirement" code:404 userInfo:nil];
        reject(@"OS requirement", @"iOS 11+ required", error);
        return;
    }

    BOOL success = [self start];

    if (success == NO)
    {
        NSError* error = [NSError errorWithDomain:@"Open safari" code:404 userInfo:nil];
        reject(@"Open safari", @"could not start opening safari", error);
    }
}

//----------------------------------------------------------------------------
- (BOOL) start
{
    if (@available(iOS 12.0, *))
    {
        self.asWebAuthSession.presentationContextProvider = self;
        return [self.asWebAuthSession start];
    }

    if (@available(iOS 11.0, *))
    {
        return [self.sfAuthSession start];
    }

    return NO;
}

//----------------------------------------------------------------------------
- (void) processResponseWithUrl:(NSURL*)callbackURL error:(NSError*)error
                       resolver:(RCTPromiseResolveBlock)resolve rejecter:(RCTPromiseRejectBlock)reject
{
    if (error != nil)
    {
        NSLog(@"%@", [error localizedDescription]);

        NSInteger errorCode = [self arcadiaErrorCodeByASWebAuthenticationSessionErrorCode:error.code];
        reject([@(errorCode) stringValue], [error localizedDescription], error);
        return;
    }

    if ([self isCancellationUrl:callbackURL])
    {
        reject([@(CANCELLATION_ERROR_CODE) stringValue], @"Back button pressed", nil);
        return;
    }

    resolve(callbackURL.absoluteString);
}

//----------------------------------------------------------------------------
- (NSInteger) arcadiaErrorCodeByASWebAuthenticationSessionErrorCode:(NSInteger)sessionErrorCode
{
    if (@available(iOS 12.0, *))
    {
        if (sessionErrorCode == ASWebAuthenticationSessionErrorCodeCanceledLogin)
        {
            return CANCELLATION_ERROR_CODE;
        }
    }

    if (@available(iOS 11.0, *))
    {
        if (sessionErrorCode == SFAuthenticationErrorCanceledLogin)
        {
            return CANCELLATION_ERROR_CODE;
        }
    }

    return sessionErrorCode;
}

//----------------------------------------------------------------------------
- (BOOL) isCancellationUrl:(NSURL*)url
{
    return [[url absoluteString] containsString:CANCELLATION_SUBCODE];
}

//----------------------------------------------------------------------------
- (ASPresentationAnchor)presentationAnchorForWebAuthenticationSession:(ASWebAuthenticationSession *)session
{
    return UIApplication.sharedApplication.keyWindow;
}

@end
