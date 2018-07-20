import { StyleSheet, Platform } from 'react-native';

export const searchViewStyles = StyleSheet.create({
    container: {
        paddingLeft: 5,
        paddingRight: 5,
        paddingTop: Platform.OS === 'ios' ? 40 : 0,
        height: Platform.OS === 'ios' ? 60 : 40,
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
    activeIcon: {
        color: 'white', 
        fontSize: 24,
    },
    inactiveIcon: {
        color: 'white', 
        fontSize: 18,
    },
    input: {
        color: 'white',
        backgroundColor: '#4bacca'
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