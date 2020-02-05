import { AppRegistry } from 'react-native';
import { Root } from './root';

const defaultHandler = ErrorUtils.getGlobalHandler && ErrorUtils.getGlobalHandler();
ErrorUtils.setGlobalHandler(function (error: any, isFatal: boolean = true) {
    console.error('Unhandled error occurred');
    defaultHandler(error, isFatal);
});

AppRegistry.registerComponent('ArcadiaAssistant', () => Root);
