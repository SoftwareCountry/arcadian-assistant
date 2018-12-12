import { StyleSheet } from 'react-native';

//----------------------------------------------------------------------------
const view = StyleSheet.create({
    modal: {
        justifyContent: 'flex-end',
        margin: 0,
    },
    background: {
        flex: 1,
        justifyContent: 'flex-end',
        backgroundColor: '#00000099',
        margin: 0,
        padding: 0,
    },
    container: {
        backgroundColor: 'white',
        alignItems: 'center',
        borderTopLeftRadius: 5,
        borderTopRightRadius: 5,
        paddingHorizontal: 24,
        paddingVertical: 24,
    },
    fingerprintImageContainer: {
        justifyContent: 'center',
        alignItems: 'center',
        width: 64,
        height: 64,
        borderRadius: 32
    },
    fingerprintImage: {
        width: 36,
        height: 36,
    },
    button: {
        width: '100%',
        padding: 5,
    },
});

//----------------------------------------------------------------------------
const text = StyleSheet.create({
    title: {
        width: '100%',
        color: '#212121',
        fontSize: 20,
        letterSpacing: 0.15,
        marginBottom: 48,
    },
    description: {
        textAlign: 'center',
        color: '#a5a5a5',
        height: 65,
        fontSize: 16,
        letterSpacing: 0.5,
        marginTop: 16,
        marginBottom: 24,
    },
    buttonText: {
        color: '#2fafcc',
        fontSize: 14,
        letterSpacing: 1.25,
    },
});

//----------------------------------------------------------------------------
export const FingerprintPopupStyle = {
    view: {
        ...view,
    },
    text: {
        ...text,
    },
};
