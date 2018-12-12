import { StyleSheet } from 'react-native';

const counterColor = '#18515E';
const labelColor = counterColor;
const backgroundColor = 'rgba(47, 175, 204, 0.2)';
const iconColor = counterColor;

//----------------------------------------------------------------------------
const counters = StyleSheet.create({
    container: {
        flex: 2,
        flexDirection: 'row',
        height: 79
    }
});

//----------------------------------------------------------------------------
const counter = StyleSheet.create({
    container: {
        backgroundColor: backgroundColor,
        flexDirection: 'column',
        flex: 1
    },
    content: {
        flex: 1,
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center'
    },
    counterContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: 10
    },
    icon: {
        paddingRight: 10,
        color: iconColor,
        opacity: .5
    },
    counter: {
        fontSize: 26,
        lineHeight: 32,
        color: counterColor
    },
    label: {
        fontSize: 10,
        lineHeight: 12,
        color: labelColor
    }
});

//----------------------------------------------------------------------------
export const StyleDays = {
    counters: {
        ...counters,
    },
    counter: {
        ...counter,
    },
};
