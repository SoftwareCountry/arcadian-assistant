import { AppRegistry, Alert } from 'react-native';
import { Root } from './root';

import 'url-search-params-polyfill';

const defaultHandler = ErrorUtils.getGlobalHandler && ErrorUtils.getGlobalHandler();
ErrorUtils.setGlobalHandler(function(error: any, isFatal: true) {
    console.error('Unhandled error ocurred');
    defaultHandler(error, isFatal);
});

AppRegistry.registerComponent('ArcadiaAssistant', () => Root);
