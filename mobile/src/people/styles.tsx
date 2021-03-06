import { Dimensions, StyleSheet } from 'react-native';
import Style from '../layout/style';

const screenHeight = Dimensions.get('window').height;
const screenWidth = Dimensions.get('window').width;

export const employeesListStyles = StyleSheet.create({
    company: {
        backgroundColor: Style.color.white
    },
    view: {
        backgroundColor: Style.color.white
    },
});

export const employeesListItemStyles = StyleSheet.create({
    layout: {
        height: 80,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center'
    },
    avatarContainer: {
        width: 72,
        height: 72,
        marginLeft: 12,
    },
    avatarOuterFrame: {
        borderWidth: 1
    },
    avatarImage: {
        borderWidth: 2
    },
    info: {
        flex: 1,
        marginRight: 12,
        marginLeft: 12,
        flexDirection: 'column',
        justifyContent: 'center'
    },
    baseText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 10,
        paddingTop: 2
    },
    name: {
        fontFamily: 'Helvetica-Light',
        fontSize: 15,
        paddingBottom: 2
    }
});

export const nodesContainerHeight = screenHeight * 0.105;
export const nodesContainerWidth = screenWidth;

export const companyDepartments = StyleSheet.create({
    levelContainer: {
        flex: 1,
        backgroundColor: Style.color.white,
        borderTopWidth: 1,
        borderColor: 'rgba(0, 0, 0, 0.15)'
    },
    nodesContainer: {
        position: 'relative',
        height: nodesContainerHeight
    },
    nodesSwipeableContainer: {
        flex: 1,
        flexDirection: 'row',
        position: 'absolute'
    }
});

const contentMargin = screenHeight * 0.018;

export const companyDepartmentsAnimatedNode = StyleSheet.create({
    container: {
        flexDirection: 'row',
    },
    stickyContainer: {
        justifyContent: 'center',
        alignItems: 'center',
        zIndex: 1,
        alignSelf: 'center',
    },
    scaleContainer: {
        justifyContent: 'center',
        alignItems: 'center'
    },
    content: {
        marginLeft: contentMargin,
        justifyContent: 'space-between',
        marginTop: contentMargin,
        marginBottom: contentMargin
    },
    name: {
        fontFamily: 'Helvetica-Light',
        fontSize: 15,
    },
    contentPosition: {
        fontFamily: 'Helvetica-Light',
        fontSize: 10,
    },
    contentDepartmentAbbreviation: {
        fontSize: 9,
        color: Style.color.base,
    }
});

export const companyDepartmentLevelPeople = StyleSheet.create({
    list: {
        flex: 1,
        paddingLeft: screenHeight * 0.02,
        paddingRight: screenHeight * 0.02
    },
    listItem: {
        flexDirection: 'row',
        flex: 1,
        height: screenHeight * 0.07,
        marginTop: screenHeight * 0.005,
        marginBottom: screenHeight * 0.005
    },
    listItemAvatar: {
        flex: 1,
        marginLeft: contentMargin,
    },
    listItemContent: {
        marginLeft: contentMargin,
        flex: 5,
        alignItems: 'flex-start',
        alignSelf: 'center',
    },
    listItemName: {
        fontFamily: 'Helvetica-Light',
        fontSize: 15,
    },
    listItemPosition: {
        fontFamily: 'Helvetica-Light',
        fontSize: 10
    }
});

export const avatarStyles = StyleSheet.create({
    container: {
        width: '100%',
        height: '100%',
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center'
    },
    outerFrame: {
        borderColor: Style.color.base,
        borderWidth: 1
    },
    image: {
        flex: 1,
        borderColor: Style.color.white,
        justifyContent: 'center',
        alignItems: 'center'
    },
});
