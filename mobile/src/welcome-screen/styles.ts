import { StyleSheet } from 'react-native';
import Style from '../layout/style';

export const welcomeScreenStyles = StyleSheet.create({
    container: {
        backgroundColor: Style.color.base,
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center'
    },
    greeting: {
        color: Style.color.white,
        fontSize: 26,
        textAlign: 'center',
    },
    loginText: {
        color: Style.color.base,
        fontSize: 22,
        textAlign: 'center',
    },
    loginButtonContainer: {
        marginTop: 140,
        borderRadius: 50,
        backgroundColor: Style.color.white,
        width: 250
    }
});
