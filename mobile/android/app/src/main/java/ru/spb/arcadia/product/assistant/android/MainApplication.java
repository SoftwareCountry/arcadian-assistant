package ru.spb.arcadia.product.assistant.android;

import android.app.Application;

import com.facebook.react.ReactApplication;
import com.reactnativecommunity.asyncstorage.AsyncStoragePackage;
import com.dylanvann.fastimage.FastImageViewPackage;
import com.swmansion.gesturehandler.react.RNGestureHandlerPackage;
import com.facebook.react.ReactNativeHost;
import com.facebook.react.ReactPackage;
import com.facebook.react.shell.MainReactPackage;
import com.facebook.soloader.SoLoader;
import com.hieuvp.fingerprint.ReactNativeFingerprintScannerPackage;
import com.lugg.ReactNativeConfig.ReactNativeConfigPackage;
import com.microsoft.appcenter.reactnative.analytics.AppCenterReactNativeAnalyticsPackage;
import com.microsoft.appcenter.reactnative.appcenter.AppCenterReactNativePackage;
import com.microsoft.appcenter.reactnative.crashes.AppCenterReactNativeCrashesPackage;
import com.microsoft.appcenter.reactnative.push.AppCenterReactNativePushPackage;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.oblador.vectoricons.VectorIconsPackage;

import java.util.Arrays;
import java.util.List;

import br.com.classapp.RNSensitiveInfo.RNSensitiveInfoPackage;

//============================================================================
public class MainApplication extends Application implements ReactApplication {
    //----------------------------------------------------------------------------
    static {
        AppCenterReactNativeShared.setAppSecret(BuildConfig.appCenterSecretId);
        AppCenterReactNativeShared.setStartAutomatically(true);
    }

    //----------------------------------------------------------------------------
    private final ReactNativeHost mReactNativeHost = new ReactNativeHost(this) {
        @Override
        public boolean getUseDeveloperSupport() {
            return BuildConfig.DEBUG;
        }

        @Override
        protected List<ReactPackage> getPackages() {
            return Arrays.asList(
                    new MainReactPackage(),
                    new AsyncStoragePackage(),
                    new FastImageViewPackage(),
                    new RNGestureHandlerPackage(),
                    new AppCenterReactNativePushPackage(MainApplication.this),
                    new ReactNativeConfigPackage(),
                    new ReactNativeFingerprintScannerPackage(),
                    new VectorIconsPackage(),
                    new RNSensitiveInfoPackage(),
                    new AppCenterReactNativeCrashesPackage(MainApplication.this, getResources().getString(R.string.appCenterCrashes_whenToSendCrashes)),
                    new AppCenterReactNativeAnalyticsPackage(MainApplication.this, getResources().getString(R.string.appCenterAnalytics_whenToEnableAnalytics)),
                    new AppCenterReactNativePackage(MainApplication.this),
                    new AuthenticationSessionPackage()
            );
        }

        @Override
        protected String getJSMainModuleName() {
            return "artifacts/index";
        }
    };

    //----------------------------------------------------------------------------
    @Override
    public ReactNativeHost getReactNativeHost() {
        return mReactNativeHost;
    }

    //----------------------------------------------------------------------------
    @Override
    public void onCreate() {
        super.onCreate();
        SoLoader.init(this, /* native exopackage */ false);
    }

}
