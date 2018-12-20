import { StyleSheet } from 'react-native';
import Style from '../../layout/style';

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

export const noResult = StyleSheet.create({
    container: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
    },
    text: {
        fontSize: 20,
        color: Style.color.base,
        marginTop: 20,
        fontFamily: 'CenturyGothic',
        textAlign: 'center',
    },
});
