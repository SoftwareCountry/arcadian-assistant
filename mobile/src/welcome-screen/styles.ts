import { StyleSheet } from 'react-native';

export const welcomeScreenColor = '#2FAFCC';
export const welcomeScreenStyles = StyleSheet.create({
    container: {
        backgroundColor: welcomeScreenColor,
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center'
    },
    greeting: {
        color: '#fff',
        fontSize: 26,
        paddingBottom: 100,
        textAlign: 'center',
    },
    loginButtonContainer: {
        borderRadius: 50,
        backgroundColor: '#fff',
        width: 250
    }
});
