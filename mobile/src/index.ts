import { AppRegistry, Alert } from 'react-native';
import { Root } from './app';

const defaultHandler = ErrorUtils.getGlobalHandler && ErrorUtils.getGlobalHandler();
ErrorUtils.setGlobalHandler(function(error: any, isFatal: true) {
    console.error("Unhandled error ocurred");
    defaultHandler.apply(this, arguments);
});

AppRegistry.registerComponent('ArcadiaAssistant', () => Root);
