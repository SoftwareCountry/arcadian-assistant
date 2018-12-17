package ru.spb.arcadia.product.assistant.android;

import android.content.Intent;
import android.net.Uri;
import android.support.customtabs.CustomTabsIntent;

import com.facebook.react.bridge.ActivityEventListener;
import com.facebook.react.bridge.BaseActivityEventListener;
import com.facebook.react.bridge.LifecycleEventListener;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;

/**
 * Created by alexander.shevnin on 31/03/2018.
 */

public class AuthenticationSessionModule extends ReactContextBaseJavaModule implements LifecycleEventListener {

    private static final int CANCELLATION_ERROR_CODE = 1;
    private static final String CANCELLATION_SUBCODE = "error_subcode=cancel";

    private Promise currentSession;
    private Uri requestedRedirectUri;

    private void cleanUp() {
        this.currentSession = null;
        this.requestedRedirectUri = null;
    }

    private ActivityEventListener mEventListener = new BaseActivityEventListener(){
        @Override
        public void onNewIntent(Intent intent) {
            super.onNewIntent(intent);
            tryToProcessLogin(intent);
        }
    };


    public AuthenticationSessionModule(ReactApplicationContext reactContext) {
        super(reactContext);

        reactContext.addLifecycleEventListener(this);
        reactContext.addActivityEventListener(this.mEventListener);
    }

    @Override
    public String getName() {
        return "AuthenticationSession";
    }

    @ReactMethod
    public void start(String uri, String redirectUri, Promise promise) {

        if (this.currentSession != null) {
            promise.reject(new Exception("The session is already in progress"));
            return;
        }

        try {
            this.requestedRedirectUri = Uri.parse(redirectUri);
            this.currentSession = promise;

            CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
            builder.setShowTitle(false);
            CustomTabsIntent customTabsIntent = builder.build();
            //customTabsIntent.intent.setPackage(CHROME_PACKAGE_NAME);
            customTabsIntent.intent.addFlags(Intent.FLAG_ACTIVITY_NO_HISTORY);
            customTabsIntent.intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
            customTabsIntent.launchUrl(getCurrentActivity(), Uri.parse(uri));
        }
        catch (Exception e){
            promise.reject(e);
            this.cleanUp();
        }
    }

    @ReactMethod
    public void reset() {
        if (this.currentSession != null) {
            this.currentSession.reject(String.valueOf(CANCELLATION_ERROR_CODE), "The session was reset");
        }

        this.cleanUp();
    }

    @Override
    public void onHostResume() {
        this.reset();
    }

    @Override
    public void onHostPause() {

    }

    @Override
    public void onHostDestroy() {
    }

    private void tryToProcessLogin(Intent intent) {
        if (this.currentSession == null) {
            return;
        }

        Uri data = intent.getData();
        if (data == null) {
            return;
        }

        try {

            if (!data.getScheme().equals(this.requestedRedirectUri.getScheme())
                    || !data.getHost().equals(this.requestedRedirectUri.getHost())) {
                throw new Exception(String.format("Received unknown redirect url %s", data.toString())); //unknown url
            }

            String url = data.toString();

            if (isCancellationUrl(url)) {
                this.currentSession.reject(String.valueOf(CANCELLATION_ERROR_CODE), "Back button on authorization screen presses");
            } else {
                this.currentSession.resolve(url);
            }

            cleanUp();

        } catch(Exception e) {
            this.currentSession.reject(e);
            cleanUp();
        }
    }

    private boolean isCancellationUrl(String url) {
        return url.contains(CANCELLATION_SUBCODE);
    }
}
