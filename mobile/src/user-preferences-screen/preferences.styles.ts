import { StyleSheet } from 'react-native';

export const preferencesStyles = StyleSheet.create({
    container: {
        flex: 1,
    },
    settingsView: {
        alignItems: 'flex-start',
        paddingLeft: 10
    },
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
    headerTitle: {
        fontFamily: 'CenturyGothic',
        fontSize: 14,
        color: 'white',
        textAlign: 'center',
    },
    icon: {
        color: 'white',
        fontSize: 26,
    }
});

