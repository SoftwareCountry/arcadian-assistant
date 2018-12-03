import React from 'react';
import { Text, TouchableOpacity, View } from 'react-native';
import { Avatar } from '../people/avatar';
import { Feed } from '../reducers/feeds/feed.model';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { FeedStyle } from './home-feeds-screen.styles';
import { Nullable } from 'types';

//============================================================================
interface FeedMessageProps {
    message: Feed;
    employee?: Employee;
    onAvatarClicked?: (e: Employee) => void;
}

//============================================================================
export class FeedMessage extends React.Component<FeedMessageProps> {

    //----------------------------------------------------------------------------
    public render() {
        const message = this.props.message;
        const employee = this.props.employee;
        const photo = employee ? employee.photoUrl : undefined;
        const formattedDate = message.datePosted.format('MMMM D, YYYY');

        const isDisabledClick = !this.props.employee;

        const avatarContent = isDisabledClick ?
            <Avatar photoUrl={photo}/> :
            <TouchableOpacity onPress={this.onAvatarClicked} style={FeedStyle.touchableOpacityContainer}>
                <Avatar photoUrl={photo}/>
            </TouchableOpacity>;

        return (
            <View>
                <View style={FeedStyle.layout}>
                    <View style={FeedStyle.imgContainer}>
                        {avatarContent}
                    </View>
                    <View style={FeedStyle.info}>
                        <StyledText style={FeedStyle.title}>{this.props.message.title}</StyledText>
                        <StyledText style={FeedStyle.text}>{this.props.message.text}</StyledText>
                        <StyledText style={FeedStyle.date}>{formattedDate}</StyledText>
                    </View>
                </View>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private onAvatarClicked = () => {
        if (this.props.onAvatarClicked && this.props.employee) {
            this.props.onAvatarClicked(this.props.employee);
        }
    };
}
