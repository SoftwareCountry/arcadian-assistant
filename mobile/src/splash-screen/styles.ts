import { StyleSheet } from 'react-native';
import Style from '../layout/style';

export const splashScreenStyles = StyleSheet.create({
    container: {
        backgroundColor: Style.color.base,
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center'
    },
    imageContainer: {
        width: '50%',
        height: '50%',
        justifyContent: 'center',
        alignItems: 'center',
    },
});
