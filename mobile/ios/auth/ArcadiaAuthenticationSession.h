//
// Created by Tsimokha, Yaroslav on 2018-12-13.
// Copyright (c) 2018 JSC Arcadia Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <React/RCTBridgeModule.h>

@import SafariServices;

//============================================================================
@interface ArcadiaAuthenticationSession : NSObject <RCTBridgeModule>

@property (nonatomic) ASWebAuthenticationSession * authSession;

@end