import { AppRegistry } from 'react-native';
import { Root } from './app';

//uncomment for Network debugging in Chrome
//declare const GLOBAL: any;
//GLOBAL.XMLHttpRequest = GLOBAL.originalXMLHttpRequest || GLOBAL.XMLHttpRequest;

AppRegistry.registerComponent('ArcadiaAssistant', () => Root);
