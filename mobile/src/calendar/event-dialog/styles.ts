import { StyleSheet } from 'react-native';

export const layout = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#18515E',
        alignSelf: 'stretch',
        flexDirection: 'row'
    },
    icon: {
        flex: 3,
        alignItems: 'center',
        paddingTop: 25
    },
    content: {
        flex: 6,
        paddingTop: 25,
        alignSelf: 'stretch'
    },
    close: {
        flex: 1,
        alignItems: 'flex-end'
    },
    text: {
        flex: 1
    },
    buttons: {
        flexDirection: 'row',
        paddingTop: 5,
        paddingBottom: 15
    }
});

const textColor = '#fff';
export const content = StyleSheet.create({
    icon: {
        color: textColor,
        fontSize: 70
    },
    title: {
        color: textColor,
        fontSize: 14,
        paddingLeft: 10
    },
    text: {
        color: textColor,
        fontSize: 10,
        paddingLeft: 10,
        paddingTop: 13
    }
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
        paddingTop: 15
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
        flex: 1,
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