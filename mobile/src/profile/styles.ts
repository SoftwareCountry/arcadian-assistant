import { StyleSheet } from 'react-native';

export const chevronColor = '#2FAFCC';

const circleDiameter = 150;
const chevronHeight = 50;
const placeholderHeight = circleDiameter * .5 - chevronHeight * .5;

const avatarContainerZIndex = 2;

export const chevronStyles = StyleSheet.create({
    container: {
        height: chevronHeight
    },
    chevron: {
        width: '100%',
        backgroundColor: 'transparent',
        height: 0,
        zIndex: avatarContainerZIndex,
        borderBottomWidth: chevronHeight,
        borderStyle: 'solid',
        borderLeftColor: 'transparent',
        borderRightColor: 'transparent',
        borderBottomColor: chevronColor,
        position: 'absolute',
        transform: [
            { rotate: '180deg' }
        ]
    }
});

export const layoutStyles = StyleSheet.create({
    container: {
        flex: 1,
        alignSelf: 'stretch'
    },
    content: {
        flex: 1,
        flexDirection: 'column',
        marginTop: chevronHeight + placeholderHeight

    },
    avatarContainer: {
        borderRadius: circleDiameter / 2,
        height: circleDiameter,
        width: circleDiameter,
        zIndex: avatarContainerZIndex + 1,
        left: '50%',
        top: placeholderHeight * -1,
        position: 'absolute',
        transform: [{ translateX: circleDiameter * -.5 }],
        alignItems: 'center',
        justifyContent: 'center'
    },
    chevronPlaceholder: {
        height: placeholderHeight,
        backgroundColor: chevronColor
    }
});

export const contentStyles = StyleSheet.create({
    name: {
        fontSize: 36,
        textAlign: 'center',
        color: '#000'
    },
    position: {
        fontSize: 13,
        textAlign: 'center',
        color: '#000'
    },
    department: {
        fontSize: 13,
        textAlign: 'center',
        color: '#2FAFCC',
        fontWeight: 'bold'
    },
    infoContainer: {
        flexDirection: 'row',
        padding: 5
    },
    contactsContainer: {
        paddingTop: 20,
        padding: 5,
        alignItems: 'center',
        justifyContent: 'center'
    }
});

export const tileStyles = StyleSheet.create({
    container: {
        padding: 1,
        width: '25%'
    },
    tile: {
        backgroundColor: 'rgba(47, 175, 204, 0.2)',
        paddingBottom: 8,
        paddingTop: 10,
        flexDirection: 'column',
        height: 66
    },
    iconContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    icon: {
    },
    iconBirthDay: {
        width: 18
    },
    iconHireDate: {
        width: 29
    },
    iconRoom: {
        width: 25
    },
    iconOrganization: {
        width: 28
    },
    text: {
        fontSize: 11,
        textAlign: 'center',
        color: '#18515E',
        paddingTop: 2
    }
});

export const contactStyles = StyleSheet.create({
    container: {
        flexDirection: 'row',
        paddingBottom: 10
    },
    iconContainer: {
        alignItems: 'center',
        justifyContent: 'center'
    },
    icon: {
        width: 36,
        height: 32
    },
    textContainer: {
        paddingLeft: 20,
        flexDirection: 'column'
    },
    text: {
        fontSize: 14,
        lineHeight: 17,
        color: '#333'
    },
    title: {
        paddingBottom: 3,
        fontSize: 10,
        lineHeight: 12,
        color: 'rgba(0, 0, 0, 0.7)'
    }
});