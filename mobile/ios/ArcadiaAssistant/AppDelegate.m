/**
 * Copyright (c) 2015-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

#import <UserNotifications/UserNotifications.h>
#import <AppCenterReactNativeShared/AppCenterReactNativeShared.h>
#import <AppCenterReactNativePush/AppCenterReactNativePush.h>
#import <AppCenterReactNativeCrashes/AppCenterReactNativeCrashes.h>
#import <AppCenterReactNativeAnalytics/AppCenterReactNativeAnalytics.h>
#import <AppCenterReactNative/AppCenterReactNative.h>
#import <ReactNativeConfig/ReactNativeConfig.h>
#import <React/RCTBundleURLProvider.h>
#import <React/RCTRootView.h>
#import <React/RCTLinkingManager.h>

#import "AppDelegate.h"

@import AppCenterPush;
@import AppCenterReactNativeShared;

//============================================================================
@interface AppDelegate (UNUserNotificationCenterDelegate) <UNUserNotificationCenterDelegate>
@end

//============================================================================
@implementation AppDelegate

//----------------------------------------------------------------------------
- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    [UNUserNotificationCenter currentNotificationCenter].delegate = self;
    
    NSString* appSecret = [ReactNativeConfig envFor:@"appCenterSecretId"];
    [AppCenterReactNativeShared setAppSecret:appSecret];
    [AppCenterReactNativeShared setStartAutomatically:YES];
    [AppCenterReactNative register];  // Initialize AppCenter
    [AppCenterReactNativePush register];  // Initialize AppCenter push
    [AppCenterReactNativeCrashes registerWithAutomaticProcessing];  // Initialize AppCenter crashes
    [AppCenterReactNativeAnalytics registerWithInitiallyEnabled:true];  // Initialize AppCenter analytics

    NSURL* jsCodeLocation;
    #if DEBUG
    jsCodeLocation = [[RCTBundleURLProvider sharedSettings] jsBundleURLForBundleRoot:@"index.ios" fallbackResource:nil];
    #else
    jsCodeLocation = [[NSBundle mainBundle] URLForResource:@"main" withExtension:@"jsbundle"];
    #endif

    RCTRootView* rootView = [[RCTRootView alloc] initWithBundleURL:jsCodeLocation
                                                        moduleName:@"ArcadiaAssistant"
                                                 initialProperties:nil
                                                     launchOptions:launchOptions];
    rootView.backgroundColor = [[UIColor alloc] initWithRed:1.0f green:1.0f blue:1.0f alpha:1];

    UIView *launchScreenView = [[NSBundle mainBundle] loadNibNamed:@"LaunchScreen" owner:self options:nil][0];
    launchScreenView.frame = self.window.bounds;
    rootView.loadingView = launchScreenView;

    self.window = [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];
    UIViewController *rootViewController = [UIViewController new];
    rootViewController.view = rootView;
    self.window.rootViewController = rootViewController;
    [self.window makeKeyAndVisible];
    return YES;
}

//----------------------------------------------------------------------------
- (BOOL)application:(UIApplication *)application
            openURL:(NSURL *)url
            options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options
{
    return [RCTLinkingManager application:application openURL:url options:options];
}

//----------------------------------------------------------------------------
- (void)application:(UIApplication *)application
didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    // Pass the device token to MSPush.
    [MSPush didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

//----------------------------------------------------------------------------
- (void)application:(UIApplication *)application
didFailToRegisterForRemoteNotificationsWithError:(nonnull NSError *)error
{
    // Pass the error to MSPush.
    [MSPush didFailToRegisterForRemoteNotificationsWithError:error];
}

@end

//============================================================================
@implementation AppDelegate (UNUserNotificationCenterDelegate)

//----------------------------------------------------------------------------
- (void)userNotificationCenter:(UNUserNotificationCenter *)center
didReceiveNotificationResponse:(UNNotificationResponse *)response
         withCompletionHandler:(void (^)(void))completionHandler API_AVAILABLE(ios(10.0))
{
    // Pass the notification payload to MSPush.
    [MSPush didReceiveRemoteNotification:response.notification.request.content.userInfo];
    
    // Complete handling the notification.
    completionHandler();
}

@end
