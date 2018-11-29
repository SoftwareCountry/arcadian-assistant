import { StyleSheet } from 'react-native';

export const radioButtonsGroupStyles = StyleSheet.create({
    container: {
        flex: 1,
    },
    settingsView: {
        alignItems: 'flex-start',
        paddingLeft: 10
    },
    scrollView: {},
    switchSettingContainer: {
        flex: 1,
        height: 60,
        flexDirection: 'row',
        justifyContent: 'space-between',
    },
    switchControllerContainer: {
        marginRight: 20,
        alignSelf: 'center',
    },
    switchSettingTitle: {
        fontFamily: 'CenturyGothic',
        fontSize: 16,
        textAlign: 'left',
        color: '#000',
        marginLeft: 20,
        alignSelf: 'center',
    },
    pickerSettingContainer: {
        flex: 1,
        height: 200,
    },
    pickerSettingTitle: {
        fontFamily: 'CenturyGothic',
        fontSize: 16,
        textAlign: 'left',
        color: '#000',
        marginLeft: 20,
        marginTop: 10,
        marginBottom: 20,
    },
    pickerLabel: {
        fontFamily: 'CenturyGothic',
        fontSize: 14,
    },
    headerTitle: {
        fontFamily: 'CenturyGothic',
        fontSize: 14,
        color: 'white',
        textAlign: 'center',
    },
    radioButtonContainer: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    radioButtonBorder: {
        borderWidth: 2,
        justifyContent: 'center',
        alignItems: 'center',
    },
});
