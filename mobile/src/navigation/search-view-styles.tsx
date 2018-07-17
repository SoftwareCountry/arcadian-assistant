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
    activeIcon: {
        color: 'white', 
        fontSize: 24,
    },
    inactiveIcon: {
        color: 'white', 
        fontSize: 18,
    },
    input: {
        width: '100%',
        color: 'white',
    },
    loadingContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    loadingText: {
        fontSize: 20
    }
  });