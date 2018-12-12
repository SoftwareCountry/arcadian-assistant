import { StyleSheet } from 'react-native';

export const searchViewStyles = StyleSheet.create({
    container: {
        flex: 1,
        paddingLeft: 5,
        paddingRight: 5,
        backgroundColor: '#2FAFCC',
        flexDirection: 'row',
        alignItems: 'center'
    },
    iconsContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
    },
    inputContainer: {
        flex: 10
    },
    buttonContainer: {
        flex: 3,
        alignItems: 'center',
        justifyContent: 'center',
    },
    icon: {
        color: 'white',
        fontSize: 18,
    },
    input: {
        color: 'white'
    },
    cancel: {
        color: 'white',
    },
    loadingContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    }
});
