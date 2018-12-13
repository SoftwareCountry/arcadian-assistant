/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

#import <Foundation/Foundation.h>
#import <React/RCTBridgeModule.h>

@import SafariServices;

//============================================================================
@interface ArcadiaAuthenticationSession: NSObject<RCTBridgeModule>

@property (nonatomic) ASWebAuthenticationSession* authSession;

@end
