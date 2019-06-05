import { Dimensions, StyleSheet } from 'react-native';
import Style from '../layout/style';

const circleDiameter = Dimensions.get('window').width * 0.5;
const chevronHeight = 50;
const headerGap = 20; //adds additional gap over the avatar. used mainly for ios
const placeholderHeight = circleDiameter * 0.5; //used to add background for the avatar
const paddingInfoContainer = 12;
const paddingTile = 1;


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
        borderBottomColor: Style.color.base,
        position: 'absolute',
        transform: [
            { rotate: '180deg' }
        ]
    }
});

export const layoutStyles = StyleSheet.create({
    container: {
        flex: 1,
        alignSelf: 'stretch',
        backgroundColor: Style.color.white,
    },
    content: {
        flex: 1,
        flexDirection: 'column',
        backgroundColor: Style.color.white,
        marginTop: circleDiameter * 0.50,
    },
    header: {
        alignSelf: 'center',
        marginBottom: 5,
    },
    avatarContainer: {
        borderRadius: circleDiameter / 2,
        height: circleDiameter,
        width: circleDiameter,
        zIndex: avatarContainerZIndex + 1,
        left: '50%',
        top: -circleDiameter * 0.5 + chevronHeight * 0.5,
        position: 'absolute',
        transform: [{ translateX: circleDiameter * -.5 }],
        alignItems: 'center',
        justifyContent: 'center'
    },
    chevronPlaceholder: {
        height: Math.round(placeholderHeight - chevronHeight * 0.5),
        backgroundColor: Style.color.base
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
        color: '#000',
        marginTop: 10
    },
    department: {
        fontSize: 13,
        textAlign: 'center',
        color: Style.color.base,
        fontWeight: 'bold',
        marginTop: 8
    },
    infoContainer: {
        flexDirection: 'row',
        padding: paddingInfoContainer,
        marginTop: 20
    },
    contactsContainer: {
        padding: 5,
        paddingBottom: 5,
        alignItems: 'center',
        justifyContent: 'center'
    }
});

export const tileStyles = StyleSheet.create({
    container: {
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
        color: '#18515E',
    },
    text: {
        fontSize: 11,
        textAlign: 'center',
        color: '#18515E',
        paddingTop: 2
    },
    separator: {
        flexBasis: 1,
        backgroundColor: Style.color.white
    }
});

export const contactStyles = StyleSheet.create({
    container: {
        flexDirection: 'row',
        height: 60,
        alignItems: 'center'
    },
    iconContainer: {
        width: 50,
        height: 40,
        alignItems: 'center',
        justifyContent: 'center'
    },
    icon: {
        color: '#18515E',
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

export const eventStyles = StyleSheet.create({
    container: {
        marginTop: 12,
        padding: 5,
    },
    loadingContainer : {
        height: 40,
        marginBottom: 20,
        backgroundColor: 'transparent',
        justifyContent: 'center',
        alignItems: 'center',
    },
});

