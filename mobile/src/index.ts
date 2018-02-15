import { AppRegistry, Alert } from 'react-native';
import { Root } from './app';

//uncomment for Network debugging in Chrome
//declare const GLOBAL: any;
//GLOBAL.XMLHttpRequest = GLOBAL.originalXMLHttpRequest || GLOBAL.XMLHttpRequest;

const defaultHandler = ErrorUtils.getGlobalHandler && ErrorUtils.getGlobalHandler();
ErrorUtils.setGlobalHandler(function(error: any, isFatal: true) {
    console.error('Unhandled error ocurred');
    defaultHandler(error, isFatal);
});

AppRegistry.registerComponent('ArcadiaAssistant', () => Root);
