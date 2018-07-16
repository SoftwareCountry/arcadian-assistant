import { StyleSheet, Platform } from 'react-native';

export const searchViewStyles = StyleSheet.create({
    container: {
        paddingLeft: 5,
        height: 40,
        backgroundColor: '#2FAFCC',
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
    },
    iconsContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
    },
    inputContainer: {
        flex: 10,
        alignItems: 'flex-start',
    },
    icon: {
        color: 'white', 
        fontSize: 24,
    },
    input: {
        width: '100%',
        color: 'white',
    },
    loadingContainer: {
        flex: 2,
        alignItems: 'center',
        justifyContent: 'center'
    },
    loadingTextContainer: {
        flex: 1,
        alignItems: 'flex-start',
        justifyContent: 'flex-start'
    },
    searchContainer: {
        flex: 1,
        alignItems: 'flex-start',
        justifyContent: 'flex-start'
    },
    loadingText: {
        fontSize: 20
    }
  });