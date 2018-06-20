import { StyleSheet, Platform } from 'react-native';

export const searchViewStyles = StyleSheet.create({
    container: {
        paddingTop: Platform.OS === 'ios' ? 30 : 10,
        paddingLeft: Platform.OS === 'ios' ? 30 : 10,
        height: Platform.OS === 'ios' ? 80 : 50,
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
        fontSize: 28,
    },
    input: {
        width: '100%',
        color: 'white',
    },
  });