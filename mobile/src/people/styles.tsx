import { StyleSheet, Dimensions } from 'react-native';

const screenHeight = Dimensions.get('window').height;
const screenWidth = Dimensions.get('window').width;

export const employeesListStyles  = StyleSheet.create({
    company: {
        backgroundColor: '#FFF'
    },
    view: {
        paddingTop: 5,
        backgroundColor: '#FFF'
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
        fontSize: 9,
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
        backgroundColor: '#fff',
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
        zIndex: 1
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
    contentPosition: {
        fontSize: 10
    },
    contentDepartmentAbbreviation: {
        fontSize: 9,
        color: '#2FAFCC',        
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
        height: screenHeight * 0.052,
        marginTop: screenHeight * 0.005,
        marginBottom: screenHeight * 0.005
    },
    listItemAvator: {
        flex: 1
    },
    listItemContent: {
        flex: 5,
        flexDirection: 'row',
        alignItems: 'center'
    },
    listItemName: {
        fontSize: 11,
        fontWeight: '600'
    },
    listItemPosition: {
        fontSize: 11
    }    
});