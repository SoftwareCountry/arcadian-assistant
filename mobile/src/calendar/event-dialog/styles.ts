import { StyleSheet } from 'react-native';
import Style from '../../layout/style';

export const layout = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#18515E',
        alignSelf: 'stretch',
        flexDirection: 'row',
    },
    icon: {
        flex: 2,
        alignSelf: 'center',
        padding: 5,
    },
    content: {
        flex: 7,
        flexDirection: 'column',
        paddingTop: 10,
        paddingBottom: 10,
        alignSelf: 'stretch',
        justifyContent: 'space-between',
    },
    close: {
        flex: 1,
        alignItems: 'center',
        height: 60,
        width: 60,
    },
    text: {
        flex: 1,
    },
});

export const content = StyleSheet.create({
    icon: {
        color: Style.color.white,
        fontSize: 50
    },
    title: {
        color: Style.color.white,
        fontSize: 14,
        paddingLeft: 10,
        paddingRight: 10,
    },
    text: {
        color: Style.color.white,
        fontSize: 11,
        paddingTop: 5,
        paddingBottom: 5,
        paddingLeft: 10,
        paddingRight: 10,
    },
    buttons: {
        flexDirection: 'row',
        paddingTop: 5,
        paddingBottom: 5,
        paddingLeft: 10,
        paddingRight: 10,
    },
});

const defaultButtonStyle = StyleSheet.create({
    border: {
        height: 30,
        borderColor: '#56CCF2'
    },
    label: {
        color: '#56CCF2',
        fontSize: 12
    }
});

export const buttons = StyleSheet.create({
    close: {
        fontSize: 50,
        lineHeight: 50,
        color: 'rgba(255, 255, 255, 0.5)',
        paddingRight: 5,
        paddingTop: 15,
    },
    cancel: StyleSheet.flatten([defaultButtonStyle.border, {
        marginRight: 5
    }]),
    accept: StyleSheet.flatten([defaultButtonStyle.border, {
        backgroundColor: '#56CCF2',
        marginLeft: 5
    }]),
    cancelLabel: StyleSheet.flatten([defaultButtonStyle.label]),
    acceptLabel: StyleSheet.flatten([defaultButtonStyle.label, {
        color: '#18515E'
    }]),
    progressIndicator: {
        position: 'absolute',
        right: '-12%',
        top: '32%'
    }
});

export const switchDayoffTypeStyles = StyleSheet.create({
    container: {
        marginTop: 5,
        marginBottom: 5,
        flex: 1,
        alignSelf: 'center',
    },
    intervalBoundaries: {
        flex: 1,
        flexDirection: 'row'
    },
    intervalBoundary: {
        position: 'relative',
        left: 0
    }
});

export const dayOffDialogStyles = StyleSheet.create({
    container: {
        flex: 1,
    },
    labelStyle: {
        fontFamily: 'CenturyGothic',
        fontSize: 12,
        color: 'white',
    },
});
