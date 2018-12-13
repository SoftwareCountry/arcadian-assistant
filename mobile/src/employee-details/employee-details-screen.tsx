import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { LayoutChangeEvent, RefreshControl, SafeAreaView, View, ViewStyle } from 'react-native';
import { EmployeeDetails } from './employee-details';
import { employeeDetailsStyles } from './styles';
import { NavigationScreenProps, ScrollView } from 'react-navigation';
import { LoadingView } from '../navigation/loading';
import Style from '../layout/style';
import { layoutStyles } from '../profile/styles';
import { Action, Dispatch } from 'redux';
import { refresh } from '../reducers/refresh/refresh.action';
import { Set } from 'immutable';

//============================================================================
interface EmployeeDetailsProps {
    departments: Set<Department>;
}

//============================================================================
interface EmployeeDetailsDispatchProps {
    refresh: () => void;
}

//============================================================================
interface EmployeeDetailsState {
    width: number;
    height: number;
}

//============================================================================
class EmployeeDetailsScreenImpl extends Component<EmployeeDetailsProps & EmployeeDetailsDispatchProps & NavigationScreenProps, EmployeeDetailsState> {

    //----------------------------------------------------------------------------
    public constructor(props: Readonly<EmployeeDetailsProps & EmployeeDetailsDispatchProps & NavigationScreenProps>) {
        super(props);

        this.state = {
            width: 0,
            height: 0,
        };
    }

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps & NavigationScreenProps, nextState: EmployeeDetailsState) {

        if (this.state.width !== nextState.width || this.state.height !== nextState.height) {
            return true;
        }

        const departments = this.props.departments;
        const nextDepartments = nextProps.departments;
        if (nextDepartments && !nextDepartments.equals(departments)) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    public render() {
        return <SafeAreaView style={Style.view.safeArea} onLayout={this.onLayout}>
            <View style={employeeDetailsStyles.container}>
                <View style={[
                    employeeDetailsStyles.topHalfView,
                    {
                        width: this.state.width,
                        height: this.state.height / 2,
                    }]}/>
                <View style={[
                    employeeDetailsStyles.bottomHalfView,
                    {
                        top: this.state.height / 2,
                        width: this.state.width,
                        height: this.state.height / 2
                    }]}/>
                {this.renderEmployeeDetails()}
            </View>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    private renderEmployeeDetails(): JSX.Element {
        const employee = this.props.navigation.getParam('employee', undefined);
        const department = this.props.departments.find(department => department.departmentId === employee.departmentId);

        return employee && department ?
            <ScrollView refreshControl={<RefreshControl tintColor={Style.color.white}
                                                        refreshing={false}
                                                        onRefresh={this.onRefresh}/>}>
                <EmployeeDetails
                    department={department}
                    employee={employee}
                    layoutStylesChevronPlaceholder={layoutStyles.chevronPlaceholder as ViewStyle}
                />
            </ScrollView> :
            <LoadingView/>;
    }

    //----------------------------------------------------------------------------
    private onLayout = (event: LayoutChangeEvent) => {
        this.setState({
            width: event.nativeEvent.layout.width,
            height: event.nativeEvent.layout.height,
        });
    };

    //----------------------------------------------------------------------------
    private onRefresh = () => {
        this.props.refresh();
    };
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): EmployeeDetailsProps => ({
    departments: state.organization ? Set(state.organization.departments) : Set(),
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>): EmployeeDetailsDispatchProps => ({
    refresh: () => dispatch(refresh()),
});

export const EmployeeDetailsScreen = connect(stateToProps, dispatchToProps)(EmployeeDetailsScreenImpl);
