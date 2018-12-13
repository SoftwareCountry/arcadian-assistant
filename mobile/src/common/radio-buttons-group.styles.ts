/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { StyleSheet } from 'react-native';

//============================================================================
const view = StyleSheet.create({
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
    pickerSettingContainer: {
        flex: 1,
        height: 200,
    },
    radioButtonContainer: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    radioButtonBorder: {
        borderWidth: 2,
        justifyContent: 'center',
        alignItems: 'center',
        alignSelf: 'center',
    },
    textRow: {
        alignSelf: 'center',
        marginLeft: 10,
    },
    textColumn: {
        alignSelf: 'center',
        marginTop: 10,
    },
    layoutRow: {
        flexDirection: 'row',
        marginHorizontal: 10,
        marginVertical: 5,
    },
    layoutColumn: {
        alignItems: 'center',
        marginHorizontal: 10,
        marginVertical: 5,
    },
});

//============================================================================
const text = StyleSheet.create({
    switchSettingTitle: {
        fontFamily: 'CenturyGothic',
        fontSize: 16,
        textAlign: 'left',
        color: '#000',
        marginLeft: 20,
        alignSelf: 'center',
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
});

const radioButtonsGroupStyles = {
    view,
    text,
};

export default radioButtonsGroupStyles;
